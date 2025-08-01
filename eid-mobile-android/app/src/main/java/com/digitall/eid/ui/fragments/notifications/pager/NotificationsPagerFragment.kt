/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.pager

import androidx.appcompat.content.res.AppCompatResources
import com.digitall.eid.R
import com.digitall.eid.databinding.FragmentNotificationsPagerBinding
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.information.InformationBottomSheetFragment
import com.google.android.material.tabs.TabLayoutMediator
import org.koin.androidx.viewmodel.ext.android.viewModel

class NotificationsPagerFragment :
    BaseFragment<FragmentNotificationsPagerBinding, NotificationsPagerViewModel>() {

    companion object {
        private const val TAG = "NotificationsPagerFragmentTag"
    }

    override fun getViewBinding() = FragmentNotificationsPagerBinding.inflate(layoutInflater)

    override val viewModel: NotificationsPagerViewModel by viewModel()

    override fun setupView() {
        binding.viewPager.adapter = NotificationsPagerAdapter(this)
        binding.toolbar.navigationClickListener = {
            viewModel.onBackPressed()
        }
        TabLayoutMediator(binding.tabLayout, binding.viewPager) { tab, position ->
            when (position) {
                0 -> {
                    tab.text = StringSource(R.string.notifications_type_channels).getString(binding.root.context)
                    tab.icon = AppCompatResources.getDrawable(binding.root.context, R.drawable.ic_channels)
                }

                1 -> {
                    tab.text = StringSource(R.string.notifications_type_notifications).getString(binding.root.context)
                    tab.icon = AppCompatResources.getDrawable(binding.root.context, R.drawable.ic_notifications)
                }
            }

        }.attach()
        binding.toolbar.setSettingsIcon(
            settingsIconRes = R.drawable.ic_information,
            settingsIconColorRes = R.color.color_white,
            settingsClickListener = { showInformationBottomSheet() }
        )
    }

    private fun showInformationBottomSheet() {
        InformationBottomSheetFragment.newInstance(content = StringSource(R.string.bottom_sheet_information_notifications_information))
            .also { bottomSheet ->
                bottomSheet.show(parentFragmentManager, null)
            }
    }

}