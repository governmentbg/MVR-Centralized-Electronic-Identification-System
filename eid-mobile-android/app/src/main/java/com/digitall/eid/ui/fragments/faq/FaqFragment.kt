package com.digitall.eid.ui.fragments.faq

import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.domain.repository.common.PreferencesRepository
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.android.ext.android.inject
import org.koin.androidx.viewmodel.ext.android.viewModel

class FaqFragment : BaseWebViewFragment<FaqViewModel>() {

    companion object {
        private const val TAG = "FaqFragmentTag"
        private val URL_TO_LOAD = "${ENVIRONMENT.urlBase}mobile/useful-information"
    }

    override val viewModel: FaqViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.faq_title

    private val preferences: PreferencesRepository by inject()

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }
}