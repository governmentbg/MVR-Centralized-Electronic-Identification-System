package com.digitall.eid.ui.fragments.administrators

import com.digitall.eid.R
import com.digitall.eid.domain.APPLICATION_LANGUAGE
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class AdministratorsFragment : BaseWebViewFragment<AdministratorsViewModel>() {

    companion object {
        private const val TAG = "AdministratorsFragmentTag"
        private val URL_TO_LOAD = "${ENVIRONMENT.urlBase}mobile/home/administrators"
    }

    override val viewModel: AdministratorsViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.tab_more_administrators

    override fun onCreated() {
        super.onCreated()
        val language = APPLICATION_LANGUAGE.type
        loadWebPage("${URL_TO_LOAD}?lang=$language")
    }
}