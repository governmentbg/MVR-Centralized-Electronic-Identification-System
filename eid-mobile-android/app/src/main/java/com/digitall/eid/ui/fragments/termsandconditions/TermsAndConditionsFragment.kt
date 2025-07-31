package com.digitall.eid.ui.fragments.termsandconditions

import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class TermsAndConditionsFragment : BaseWebViewFragment<TermsAndConditionsViewModel>() {

    companion object {
        private const val TAG = "TermsAndConditionsFragmentTag"
        private val URL_TO_LOAD = "${ENVIRONMENT.urlBase}mobile/terms-and-conditions"
    }

    override val viewModel: TermsAndConditionsViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.terms_and_conditions_title

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }
}