/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.main.tabs.more

import androidx.lifecycle.viewModelScope
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.extensions.readOnly
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.launchWithDispatcher
import com.digitall.eid.extensions.readOnly
import com.digitall.eid.extensions.setValueOnMainThread
import com.digitall.eid.mappers.main.tabs.more.MainTabMoreUiMapper
import com.digitall.eid.models.common.BannerMessage
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.MainTabsEnum
import com.digitall.eid.models.main.more.TabMoreAdapterMarker
import com.digitall.eid.models.main.more.TabMoreItems
import com.digitall.eid.ui.fragments.main.base.BaseMainTabViewModel
import com.digitall.eid.utils.SingleLiveEvent
import kotlinx.coroutines.flow.MutableStateFlow
import org.koin.core.component.inject

class MainTabMoreViewModel : BaseMainTabViewModel() {

    companion object {
        private const val TAG = "MainTabFourViewModelTag"
    }

    override var mainTabsEnum: MainTabsEnum? = MainTabsEnum.TAB_MORE

    private val mainTabMoreUiMapper: MainTabMoreUiMapper by inject()

    private val _adapterList = MutableStateFlow<List<TabMoreAdapterMarker>>(emptyList())
    val adapterList = _adapterList.readOnly()

    private val _openUrlInBrowserEvent = SingleLiveEvent<String>()
    val openUrlInBrowserEvent = _openUrlInBrowserEvent.readOnly()

    override fun onFirstAttach() {
        logDebug("onFirstAttach", TAG)
        viewModelScope.launchWithDispatcher {
            val userModel = preferences.readApplicationInfo()?.userModel
            _adapterList.emit(mainTabMoreUiMapper.map(Unit, userModel?.acr))
        }
    }

    fun onElementClicked(type: TabMoreItems) {
        when (type) {
            TabMoreItems.USER_INFORMATION -> toCitizenInformationFragment()
            TabMoreItems.USER_PROFILE_SECURITY -> toCitizenProfileSecurityFragment()
            TabMoreItems.SETUP_NOTIFICATIONS -> toNotificationsFragment()
            TabMoreItems.CHANGE_EMAIL -> toChangeUserEmailFragment()
            TabMoreItems.CHANGE_PASSWORD -> toChangeUserPasswordFragment()
            TabMoreItems.CHANGE_PHONE -> toChangeUserPhoneFragment()
            TabMoreItems.EMPOWERMENT -> toEmpowermentFragment()
            TabMoreItems.JOURNAL -> toJournalFragment()
            TabMoreItems.FAQ -> toFaqFragment()
            TabMoreItems.CONTACTS -> toContactsFragment()
            TabMoreItems.TERMS_AND_CONDITIONS -> toTermsAndConditionsFragment()
            TabMoreItems.ADMINISTRATORS -> toAdministratorsFragment()
            TabMoreItems.CENTERS_CERTIFICATION_SERVICES -> toCentersCertificationServicesFragment()
            TabMoreItems.PROVIDERS_ELECTRONIC_ADMINISTRATIVE_SERVICES -> toProvidersElectronicAdministrativeServicesFragment()
            TabMoreItems.ELECTRONIC_DELIVERY_SYSTEM -> toElectronicDeliverySystemFragment()
            TabMoreItems.ONLINE_HELP_SYSTEM -> toOnlineHelpSystem()
            TabMoreItems.PAYMENT_HISTORY -> toPaymentHistory()
            TabMoreItems.SETTINGS,
            TabMoreItems.OTHER -> showMessage(BannerMessage.error(StringSource("Not implemented")))
        }
    }

    private fun toJournalFragment() {
        logDebug("toJournalFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toJournalFlowFragment())
    }

    private fun toEmpowermentFragment() {
        logDebug("toEmpowermentFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toEmpowermentFlowFragment())
    }

    private fun toNotificationsFragment() {
        logDebug("toNotificationsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toNotificationsFlowFragment())
    }

    private fun toChangeUserEmailFragment() {
        logDebug("toChangeUserEmailFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toChangeUserEmailFlowFragment())
    }

    private fun toChangeUserPasswordFragment() {
        logDebug("toChangeUserPasswordFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toChangeUserPasswordFlowFragment())
    }

    private fun toChangeUserPhoneFragment() {
        logDebug("toChangeUserPhoneFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toChangeUserPhoneFlowFragment())
    }

    private fun toCitizenInformationFragment() {
        logDebug("toCitizenInformationFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toCitizenInformationFlowFragment())
    }

    private fun toFaqFragment() {
        logDebug("toFaqFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toFaqFlowFragment())
    }

    private fun toContactsFragment() {
        logDebug("toContactsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toContactsFlowFragment())
    }

    private fun toTermsAndConditionsFragment() {
        logDebug("toTermsAndConditionsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toTermsAndConditionsFlowFragment())
    }

    private fun toAdministratorsFragment() {
        logDebug("toTermsAndConditionsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toAdministratorsFlowFragment())
    }

    private fun toCentersCertificationServicesFragment() {
        logDebug("toTermsAndConditionsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toCentersCertificationServicesFlowFragment())
    }

    private fun toProvidersElectronicAdministrativeServicesFragment() {
        logDebug("toTermsAndConditionsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toProvidersElectronicAdministrativeServicesFlowFragment())
    }

    private fun toElectronicDeliverySystemFragment() {
        logDebug("toTermsAndConditionsFragment", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toElectronicDeliverySystemFlowFragment())
    }

    private fun toCitizenProfileSecurityFragment() {
        logDebug("toCitizenProfileSecurity", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toCitizenProfileSecurityFlowFragment())
    }

    private fun toOnlineHelpSystem() {
        logDebug("toOnlineHelpSystem", TAG)
//        navigateInTab(MainTabMoreFragmentDirections.toOnlineHelpSystemFlowFragment())
        _openUrlInBrowserEvent.setValueOnMainThread(ENVIRONMENT.urlPopop)
    }

    private fun toPaymentHistory() {
        logDebug("toPaymentHistory", TAG)
        navigateInTab(MainTabMoreFragmentDirections.toPaymentsHistoryFlowFragment())
    }

}