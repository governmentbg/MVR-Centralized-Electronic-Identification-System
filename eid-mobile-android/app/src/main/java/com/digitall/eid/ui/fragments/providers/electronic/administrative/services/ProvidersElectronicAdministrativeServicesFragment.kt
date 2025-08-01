package com.digitall.eid.ui.fragments.providers.electronic.administrative.services

import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class ProvidersElectronicAdministrativeServicesFragment :
    BaseWebViewFragment<ProvidersElectronicAdministrativeServicesViewModel>() {

    companion object {
        private const val TAG = "ProvidersElectronicAdministrativeServicesFragmentTag"
        private val URL_TO_LOAD = "${ENVIRONMENT.urlBase}mobile/home/providers"
    }

    override val viewModel: ProvidersElectronicAdministrativeServicesViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.tab_more_providers_electronic_administrative_services

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }
}