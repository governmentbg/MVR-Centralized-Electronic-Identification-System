package com.digitall.eid.ui.fragments.onlinehelpsystem

import com.digitall.eid.R
import com.digitall.eid.domain.ENVIRONMENT
import com.digitall.eid.ui.fragments.base.BaseWebViewFragment
import org.koin.androidx.viewmodel.ext.android.viewModel

class OnlineHelpSystemFragment : BaseWebViewFragment<OnlineHelpSystemViewModel>() {

    companion object {
        private const val TAG = "OnlineHelpSystemFragmentTag"
    }

    override val viewModel: OnlineHelpSystemViewModel by viewModel()

    override val showToolbar: Boolean = true

    override val showSettingsButton: Boolean = false

    override val toolbarNavigationIconRes: Int = R.drawable.ic_arrow_left

    override val toolbarNavigationTextRes: Int = R.string.online_help_system

    override fun onCreated() {
        super.onCreated()
        loadWebPage(ENVIRONMENT.urlPopop)
    }
}