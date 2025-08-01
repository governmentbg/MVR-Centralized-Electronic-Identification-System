/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.fragments.notifications.pager

import androidx.fragment.app.Fragment
import androidx.viewpager2.adapter.FragmentStateAdapter
import com.digitall.eid.databinding.FragmentNotificationChannelsBinding
import com.digitall.eid.databinding.FragmentNotificationsBinding
import com.digitall.eid.ui.fragments.base.BaseFragment
import com.digitall.eid.ui.fragments.notifications.channels.NotificationChannelsFragment
import com.digitall.eid.ui.fragments.notifications.channels.NotificationChannelsViewModel
import com.digitall.eid.ui.fragments.notifications.notifications.NotificationsFragment
import com.digitall.eid.ui.fragments.notifications.notifications.NotificationsViewModel

class NotificationsPagerAdapter(fragment: Fragment) : FragmentStateAdapter(fragment) {

    private val fragmentList = listOf(
        NotificationChannelsFragment.newInstance() as BaseFragment<FragmentNotificationChannelsBinding, NotificationChannelsViewModel>,
        NotificationsFragment.newInstance() as BaseFragment<FragmentNotificationsBinding, NotificationsViewModel>,
    )

    override fun getItemCount(): Int = fragmentList.size

    override fun createFragment(position: Int): Fragment {
        return fragmentList[position]
    }
}