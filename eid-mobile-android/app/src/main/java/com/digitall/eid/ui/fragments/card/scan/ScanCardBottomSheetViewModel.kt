package com.digitall.eid.ui.fragments.card.scan

import android.app.Activity
import android.nfc.NfcAdapter
import android.nfc.Tag
import android.nfc.TagLostException
import android.nfc.tech.IsoDep
import android.os.Bundle
import android.util.Base64
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.viewModelScope
import com.digitall.eid.R
import com.digitall.eid.domain.models.events.request.EventRequestModel
import com.digitall.eid.domain.usecase.events.LogEventUseCase
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.domain.utils.LogUtil.logError
import com.digitall.eid.eidreader.card.Card
import com.digitall.eid.eidreader.card.certificate.CertificateType
import com.digitall.eid.eidreader.card.key.CardKeyType
import com.digitall.eid.eidreader.card.service.CardService
import com.digitall.eid.eidreader.reader.EIDReader
import com.digitall.eid.eidreader.reader.exception.EIDReaderException
import com.digitall.eid.extensions.launchInScope
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.models.common.CardScanContentType
import com.digitall.eid.models.common.CardScanResult
import com.digitall.eid.models.common.CardScanScreenStates
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.BaseViewModel
import org.bouncycastle.asn1.x500.style.BCStyle
import org.bouncycastle.asn1.x500.style.IETFUtils
import org.bouncycastle.cert.jcajce.JcaX509CertificateHolder
import org.bouncycastle.jce.provider.BouncyCastleProvider
import org.koin.core.component.inject
import java.io.ByteArrayInputStream
import java.security.cert.CertificateFactory
import java.security.cert.X509Certificate

class ScanCardBottomSheetViewModel : BaseViewModel(),
    NfcAdapter.ReaderCallback {

    companion object {
        private const val TAG = "ScanCardBottomSheetViewModelTag"
        private const val NFC_FLAGS = NfcAdapter.FLAG_READER_NFC_A or
                NfcAdapter.FLAG_READER_NFC_B or
                NfcAdapter.FLAG_READER_NFC_F or
                NfcAdapter.FLAG_READER_NFC_V or
                NfcAdapter.FLAG_READER_NFC_BARCODE

        private const val TRANSCEIVER_TIMEOUT = 120000
        private const val ONLINE_SUCCESSFUL_PIN_CHANGE = "ONLINE_SUCCESSFUL_PIN_CHANGE"
        private const val ONLINE_UNSUCCESSFUL_PIN_CHANGE = "ONLINE_UNSUCCESSFUL_PIN_CHANGE"
    }

    private var nfcAdapter: NfcAdapter? = null

    @Volatile
    private var isProcessingCard = false

    private lateinit var cardContentType: CardScanContentType
    private lateinit var cardService: CardService

    private val _screenStateLiveData =
        MutableLiveData<CardScanScreenStates>(CardScanScreenStates.Scanning)
    val screenStateLiveData = _screenStateLiveData.readOnly()

    private val logEventUseCase: LogEventUseCase by inject()

    override fun onBackPressed() {
        logDebug("onBackPressed", TAG)
        if (isProcessingCard) return
        popBackStack()
    }

    fun setupContentType(cardContentType: CardScanContentType) {
        this.cardContentType = cardContentType
    }

    fun enableNfcReading(activity: Activity?) {
        activity?.let { existingActivity ->
            nfcAdapter = NfcAdapter.getDefaultAdapter(existingActivity).also { adapter ->
                val bundle = Bundle().apply {
                    putInt(NfcAdapter.EXTRA_READER_PRESENCE_CHECK_DELAY, 250)
                }
                adapter.enableReaderMode(existingActivity, this, NFC_FLAGS, bundle)
            }
        }
    }

    fun disableNfcReading(activity: Activity?) {
        activity?.let { existingActivity ->
            nfcAdapter?.disableReaderMode(existingActivity)
        }
    }

    override fun onTagDiscovered(tag: Tag?) {
        tag?.let {
            if (it.techList.asList().contains("android.nfc.tech.IsoDep")) {
                try {
                    isProcessingCard = true
                    _screenStateLiveData.setValueOnMainThread(CardScanScreenStates.Processing)
                    val eidCardReader = EIDReader(IsoDep.get(it), CardKeyType.RSA).also { reader ->
                        reader.connect()
                    }

                    if (eidCardReader.isConnected) {
                        eidCardReader.timeout = TRANSCEIVER_TIMEOUT
                        val (isCardOk, isPinActivated) = eidCardReader.isPinActivated()

                        when {
                            isCardOk && isPinActivated -> {
                                val cardData = eidCardReader.readCard()
                                val card = Card(data = cardData)
                                cardService = CardService(
                                    reader = eidCardReader,
                                    card = card
                                )

                                when (cardContentType) {
                                    is CardScanContentType.ChangePin -> changeCardPin(
                                        cardCurrentPin = (cardContentType as CardScanContentType.ChangePin).cardCurrentPin
                                            ?: return,
                                        cardNewPin = (cardContentType as CardScanContentType.ChangePin).cardNewPin
                                            ?: return,
                                        cardCan = (cardContentType as CardScanContentType.ChangePin).cardCan
                                    )

                                    is CardScanContentType.SignChallenge -> signChallenge(
                                        cardCurrentPin = (cardContentType as CardScanContentType.SignChallenge).cardCurrentPin
                                            ?: return,
                                        challenge = (cardContentType as CardScanContentType.SignChallenge).challenge
                                            ?: return,
                                        cardCan = (cardContentType as CardScanContentType.SignChallenge).cardCan
                                    )
                                }
                            }

                            isCardOk -> {
                                isProcessingCard = false
                                _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Error(
                                        StringSource(R.string.nfc_card_scan_pin_not_activated_error)
                                    )
                                )
                            }

                            else -> {
                                isProcessingCard = false
                                _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Error(
                                        StringSource(R.string.nfc_card_scan_communication_failed_error)
                                    )
                                )
                            }
                        }
                    }
                } catch (exception: Exception) {
                    logError(
                        "Reading a NFC card returned with error: {${exception.message}}",
                        exception,
                        TAG
                    )
                    isProcessingCard = false

                    when (exception) {
                        is EIDReaderException.ResponseError -> {
                            when (exception.sw1 to exception.sw2) {
                                0x63.toByte() to 0xC2.toByte() -> _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Error(
                                        StringSource(R.string.nfc_card_scan_wrong_pin_next_try_error)
                                    )
                                )

                                0x63.toByte() to 0xC1.toByte() -> _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Suspended
                                )

                                0x69.toByte() to 0x83.toByte() -> _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Error(
                                        StringSource(R.string.nfc_card_scan_block_card_error)
                                    )
                                )

                                0x69.toByte() to 0x85.toByte() -> _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Suspended
                                )

                                else -> _screenStateLiveData.setValueOnMainThread(
                                    CardScanScreenStates.Error(
                                        StringSource(R.string.nfc_card_scan_communication_failed_error)
                                    )
                                )
                            }

                            when (cardContentType) {
                                is CardScanContentType.ChangePin -> logEventUseCase.invoke(
                                    data = EventRequestModel(
                                        eventType = ONLINE_UNSUCCESSFUL_PIN_CHANGE,
                                        eventPayload = mapOf(
                                            "certificateId" to ""
                                        )
                                    )
                                ).launchInScope(viewModelScope)

                                else -> {}
                            }
                        }

                        is TagLostException -> _screenStateLiveData.setValueOnMainThread(
                            CardScanScreenStates.Error(
                                StringSource(R.string.nfc_card_scan_communication_lost_error)
                            )
                        )

                        else -> _screenStateLiveData.setValueOnMainThread(
                            CardScanScreenStates.Error(
                                StringSource(R.string.nfc_card_scan_communication_failed_error)
                            )
                        )
                    }
                }
            }
        }
    }

    private fun changeCardPin(cardCurrentPin: String, cardNewPin: String, cardCan: String? = null) {
        var isCanPaceSessionSuccess = true
        cardCan?.let { can ->
            isCanPaceSessionSuccess =
                cardService.doPace(key = can, type = CardService.PaceSessionType.CAN)
        }

        val isPinPaceSessionSuccess =
            cardService.doPace(
                key = cardCurrentPin,
                type = CardService.PaceSessionType.PIN
            )
        if (isPinPaceSessionSuccess && isCanPaceSessionSuccess) {
            val signingCertificateBytes = cardService.readSigningCertificate()

            when {
                signingCertificateBytes.isEmpty() -> changeCardPinService(
                    newPin = cardNewPin,
                    certificateSerialNumber = ""
                )

                else -> {
                    val certificateFactory =
                        CertificateFactory.getInstance("X.509", BouncyCastleProvider.PROVIDER_NAME)
                    val certificateInputStream = ByteArrayInputStream(signingCertificateBytes)
                    val certificate =
                        certificateFactory.generateCertificate(certificateInputStream) as X509Certificate
                    certificateInputStream.close()
                    JcaX509CertificateHolder(certificate).subject.getRDNs(BCStyle.SERIALNUMBER)
                        .firstOrNull()?.let { rdn ->
                            val serialNumber =
                                IETFUtils.valueToString(rdn.first.value).replace("PI:BG-", "")
                            val user = preferences.readApplicationInfo()?.userModel

                            when {
                                serialNumber == user?.eidEntityId -> changeCardPinService(
                                    newPin = cardNewPin,
                                    certificateSerialNumber = certificate.serialNumber.toString(16)
                                )

                                else -> {
                                    isProcessingCard = false
                                    logEventUseCase.invoke(
                                        data = EventRequestModel(
                                            eventType = ONLINE_UNSUCCESSFUL_PIN_CHANGE,
                                            eventPayload = mapOf(
                                                "certificateId" to certificate.serialNumber.toString(
                                                    16
                                                )
                                            )
                                        )
                                    ).launchInScope(viewModelScope)
                                    _screenStateLiveData.setValueOnMainThread(
                                        CardScanScreenStates.Error(
                                            StringSource(R.string.nfc_card_scan_pace_someone_else_eid_card_failed_error)
                                        )
                                    )
                                }
                            }

                        } ?: run {
                        isProcessingCard = false
                        logEventUseCase.invoke(
                            data = EventRequestModel(
                                eventType = ONLINE_UNSUCCESSFUL_PIN_CHANGE,
                                eventPayload = mapOf(
                                    "certificateId" to certificate.serialNumber.toString(
                                        16
                                    )
                                )
                            )
                        ).launchInScope(viewModelScope)
                        _screenStateLiveData.setValueOnMainThread(
                            CardScanScreenStates.Error(
                                StringSource(R.string.nfc_card_scan_pace_malformed_certificate_failed_error)
                            )
                        )
                    }
                }
            }
        } else {
            isProcessingCard = false
            _screenStateLiveData.setValueOnMainThread(
                CardScanScreenStates.Error(
                    StringSource(R.string.nfc_card_scan_communication_failed_error)
                )
            )
        }
    }

    private fun changeCardPinService(newPin: String, certificateSerialNumber: String) {
        if (cardService.changePin(newPin = newPin)) {
            isProcessingCard = false
            logEventUseCase.invoke(
                data = EventRequestModel(
                    eventType = ONLINE_SUCCESSFUL_PIN_CHANGE,
                    eventPayload = mapOf("certificateId" to certificateSerialNumber)
                )
            ).launchInScope(viewModelScope)
            _screenStateLiveData.setValueOnMainThread(CardScanScreenStates.Success(result = CardScanResult.PinChanged))
        } else {
            isProcessingCard = false
            _screenStateLiveData.setValueOnMainThread(
                CardScanScreenStates.Error(
                    StringSource(R.string.nfc_card_scan_communication_failed_error)
                )
            )
        }
    }

    private fun signChallenge(cardCurrentPin: String, challenge: String, cardCan: String? = null) {
        var isCanPaceSessionSuccess = true
        cardCan?.let { can ->
            isCanPaceSessionSuccess =
                cardService.doPace(key = can, type = CardService.PaceSessionType.CAN)
        }

        val isPinPaceSessionSuccess =
            cardService.doPace(
                key = cardCurrentPin,
                type = CardService.PaceSessionType.PIN
            )
        if (isPinPaceSessionSuccess && isCanPaceSessionSuccess) {
            val signedChallengeBytes = cardService.sign(challenge = challenge.toByteArray())
            val certificates = cardService.readSigningCertificateAndChain()

            _screenStateLiveData.setValueOnMainThread(
                CardScanScreenStates.Success(
                    result = CardScanResult.ChallengeSigned(
                        signature = Base64.encodeToString(signedChallengeBytes, Base64.NO_WRAP),
                        challenge = challenge,
                        certificate = Base64.encodeToString(
                            certificates[CertificateType.CITIZEN.value],
                            Base64.NO_WRAP
                        ),
                        certificateChain = certificates.filter { entry -> entry.key != CertificateType.CITIZEN.value }
                            .map { entry -> Base64.encodeToString(entry.value, Base64.NO_WRAP) }
                    )
                )
            )
        } else {
            isProcessingCard = false
            _screenStateLiveData.setValueOnMainThread(
                CardScanScreenStates.Error(
                    StringSource(R.string.nfc_card_scan_communication_failed_error)
                )
            )
        }
    }
}