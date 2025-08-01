/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.main

import androidx.annotation.IdRes
import com.digitall.eid.R

enum class MainTabsEnum(
    @param:IdRes val menuID: Int,
    @param:IdRes val navigationID: Int,
    @param:IdRes val fragmentID: Int,
) {
    TAB_HOME(
        R.id.nav_main_tab_home,
        R.navigation.nav_main_tab_home,
        R.id.mainTabHomeFragment,
    ),
    TAB_EIM(
        R.id.nav_main_tab_eim,
        R.navigation.nav_main_tab_eim,
        R.id.mainTabEIMFragment,
    ),
    TAB_REQUESTS(
        R.id.nav_main_tab_requests,
        R.navigation.nav_main_tab_requests,
        R.id.mainTabRequestsFragment,
    ),
    TAB_MORE(
        R.id.nav_main_tab_more,
        R.navigation.nav_main_tab_more,
        R.id.mainTabMoreFragment,
    );

    companion object {
        fun findNavigationIDByMenuID(@IdRes menuId: Int): Int? {
            for (tab in entries) {
                if (tab.menuID == menuId) {
                    return tab.navigationID
                }
            }
            return null
        }
    }
}