/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.certificates.all

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.DEVICES
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE
import com.digitall.eid.domain.EID_MOBILE_CERTIFICATE_KEYS
import com.digitall.eid.domain.UiDateFormats
import com.digitall.eid.domain.extensions.fromServerDateToTextDate
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.certificates.CertificateItem
import com.digitall.eid.domain.models.common.DeviceType
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.domain.utils.CryptographyHelper
import com.digitall.eid.models.certificates.all.CertificateUi
import com.digitall.eid.models.certificates.all.CertificatesElementsEnumUi
import com.digitall.eid.models.certificates.all.CertificatesSpinnerElementsEnumUi
import com.digitall.eid.models.certificates.all.CertificatesStatusEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi
import org.bouncycastle.asn1.x500.style.BCStyle
import org.bouncycastle.asn1.x500.style.IETFUtils
import org.bouncycastle.cert.jcajce.JcaX509CertificateHolder
import org.bouncycastle.jce.provider.BouncyCastleProvider
import org.koin.core.component.KoinComponent
import org.koin.core.component.inject
import java.io.ByteArrayInputStream
import java.security.cert.CertificateFactory
import java.security.cert.X509Certificate

class CertificatesUiMapper :
    BaseMapper<CertificateItem, CertificateUi>(), KoinComponent {

    companion object {
        private const val TAG = "CertificatesUiMapperTag"
    }

    private val cryptographyHelper: CryptographyHelper by inject()
    private val preferences: PreferencesRepository by inject()

    override fun map(from: CertificateItem): CertificateUi {
        return with(from) {
            val status = getEnumValue<CertificatesStatusEnum>(from.status ?: "")
                ?: CertificatesStatusEnum.UNKNOWN

            val spinnerModel = when (status) {
                CertificatesStatusEnum.ACTIVE -> CommonSpinnerUi(
                    required = false,
                    question = false,
                    selectedValue = null,
                    title = StringSource(""),
                    elementEnum = CertificatesElementsEnumUi.SPINNER_MENU,
                    list = buildList {
                        add(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                originalModel = from,
                                text = CertificatesSpinnerElementsEnumUi.SPINNER_PAUSE.title,
                                elementEnum = CertificatesSpinnerElementsEnumUi.SPINNER_PAUSE,
                                iconRes = R.drawable.ic_pause,
                                iconColorRes = R.color.color_F59E0B,
                                textColorRes = CertificatesSpinnerElementsEnumUi.SPINNER_PAUSE.colorRes,
                            )
                        )

                        add(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                originalModel = from,
                                text = CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE.title,
                                elementEnum = CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE,
                                iconRes = R.drawable.ic_cancel,
                                iconColorRes = R.color.color_BF1212,
                                textColorRes = CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE.colorRes,
                            )
                        )
                        val userModel = preferences.readApplicationInfo()?.userModel
                        val hasMobileEID = checkExistenceMobileAID()
                        val isOwnCertificate =
                            cryptographyHelper.getCertificate(EID_MOBILE_CERTIFICATE)
                                ?.let { certificate ->
                                    val certStream = ByteArrayInputStream(certificate.encoded)
                                    val certFactory = CertificateFactory.getInstance(
                                        "X.509",
                                        BouncyCastleProvider.PROVIDER_NAME
                                    )
                                    val x509Certificate =
                                        certFactory.generateCertificate(certStream) as X509Certificate
                                    certStream.close()

                                    JcaX509CertificateHolder(x509Certificate).subject.getRDNs(
                                        BCStyle.SERIALNUMBER
                                    ).firstOrNull()?.let { rdn ->
                                        val serialNumber =
                                            IETFUtils.valueToString(rdn.first.value)
                                                .replace("PI:BG-", "")
                                        serialNumber == userModel?.eidEntityId
                                    } ?: false

                                } ?: false
                        when {
                            (DEVICES.firstOrNull { device -> device.id == deviceId }?.type == DeviceType.CHIP_CARD.type && userModel?.acr == UserAcrEnum.HIGH) ||
                                    (DEVICES.firstOrNull { device -> device.id == deviceId }?.type == DeviceType.MOBILE.type && hasMobileEID && isOwnCertificate) -> add(
                                CommonSpinnerMenuItemUi(
                                    isSelected = false,
                                    originalModel = from,
                                    text = CertificatesSpinnerElementsEnumUi.SPINNER_CHANGE_PIN.title,
                                    elementEnum = CertificatesSpinnerElementsEnumUi.SPINNER_CHANGE_PIN,
                                    iconRes = R.drawable.ic_edit,
                                    iconColorRes = CertificatesSpinnerElementsEnumUi.SPINNER_CHANGE_PIN.colorRes,
                                    textColorRes = CertificatesSpinnerElementsEnumUi.SPINNER_CHANGE_PIN.colorRes,
                                )
                            )
                        }
                    }
                )

                CertificatesStatusEnum.STOPPED -> CommonSpinnerUi(
                    required = false,
                    question = false,
                    selectedValue = null,
                    title = StringSource(""),
                    elementEnum = CertificatesElementsEnumUi.SPINNER_MENU,
                    list = buildList {
                        add(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                originalModel = from,
                                text = CertificatesSpinnerElementsEnumUi.SPINNER_RESUME.title,
                                elementEnum = CertificatesSpinnerElementsEnumUi.SPINNER_RESUME,
                                iconRes = R.drawable.ic_play,
                                iconColorRes = R.color.color_018930,
                                textColorRes = CertificatesSpinnerElementsEnumUi.SPINNER_RESUME.colorRes,
                            )
                        )
                        add(
                            CommonSpinnerMenuItemUi(
                                isSelected = false,
                                originalModel = from,
                                text = CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE.title,
                                elementEnum = CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE,
                                iconRes = R.drawable.ic_cancel,
                                iconColorRes = R.color.color_BF1212,
                                textColorRes = CertificatesSpinnerElementsEnumUi.SPINNER_REVOKE.colorRes,
                            )
                        )
                    }
                )

                else -> null
            }

            CertificateUi(
                id = id!!,
                serialNumber = serialNumber ?: "Unknown",
                validityUntil = validityUntil?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                ) ?: "Unknown",
                validityFrom = validityFrom?.fromServerDateToTextDate(
                    dateFormat = UiDateFormats.WITH_TIME,
                ) ?: "Unknown",
                status = status,
                deviceType = StringSource(DEVICES.firstOrNull { device -> device.id == deviceId }?.name),
                isExpiring = expiring ?: false,
                alias = alias ?: "Unknown",
                spinnerModel = spinnerModel
            )
        }
    }

    private fun checkExistenceMobileAID(): Boolean {
        val hasKeys = cryptographyHelper.hasAlias(alias = EID_MOBILE_CERTIFICATE_KEYS)
        val hasCertificate = cryptographyHelper.hasAlias(alias = EID_MOBILE_CERTIFICATE)
        val hasCertificatePin = preferences.readApplicationInfo()?.certificatePin.isNullOrEmpty()

        return hasKeys && hasCertificate && hasCertificatePin
    }
}