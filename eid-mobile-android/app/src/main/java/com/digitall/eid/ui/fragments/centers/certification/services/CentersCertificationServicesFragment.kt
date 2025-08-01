package com.digitall.eid.ui.fragments.centers.certification.services

import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class CentersCertificationServicesFragment : BaseWebViewFragment<CentersCertificationServicesViewModel>() {

    companion object {
        private const val TAG = "CentersCertificationServicesFragmentTag"
        private val URL_TO_LOAD = "${ENVIRONMENT.urlBase}mobile/home/centers"
    }

    override val viewModel: CentersCertificationServicesViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.tab_more_centers_certification_services

    private val preferences: PreferencesRepository by inject()

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }
}