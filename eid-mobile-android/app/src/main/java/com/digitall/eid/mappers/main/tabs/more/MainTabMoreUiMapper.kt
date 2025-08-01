/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.main.tabs.more

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.models.user.UserAcrEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.main.more.TabMoreAdapterMarker
import com.digitall.eid.models.main.more.TabMoreItemUi
import com.digitall.eid.models.main.more.TabMoreItems
import com.digitall.eid.models.main.more.TabMoreSeparatorUi
import com.digitall.eid.models.main.more.TabMoreTitleUi

class MainTabMoreUiMapper :
    BaseMapperWithData<Unit, UserAcrEnum, List<TabMoreAdapterMarker>>() {

    override fun map(from: Unit, data: UserAcrEnum?): List<TabMoreAdapterMarker> {
        return buildList {
            add(
                TabMoreTitleUi(
                    itemText = StringSource(R.string.tab_more_profile_section_title),
                    itemImageRes = R.drawable.ic_profile
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.USER_INFORMATION,
                    itemText = StringSource(R.string.tab_more_profile_section_information),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.USER_PROFILE_SECURITY,
                    itemText = StringSource(R.string.tab_more_profile_section_security),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.SETUP_NOTIFICATIONS,
                    itemText = StringSource(R.string.tab_more_profile_section_notifications_settings),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.PAYMENT_HISTORY,
                    itemText = StringSource(R.string.tab_more_profile_section_payment_history),
                )
            )
            add(
                TabMoreTitleUi(
                    itemText = StringSource(R.string.tab_more_services_section_title),
                    itemImageRes = R.drawable.ic_menu
                )
            )
            if (data == UserAcrEnum.HIGH) {
                add(
                    TabMoreItemUi(
                        type = TabMoreItems.EMPOWERMENT,
                        itemText = StringSource(R.string.tab_more_services_section_empowerment_register),
                    )
                )
            }
            add(
                TabMoreItemUi(
                    type = TabMoreItems.JOURNAL,
                    itemText = StringSource(R.string.tab_more_services_section_journals),
                )
            )

            add(
                TabMoreSeparatorUi(marginLeft = 32, marginRight = 32)
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.FAQ,
                    itemText = StringSource(R.string.tab_more_useful_section_faq),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.CONTACTS,
                    itemText = StringSource(R.string.tab_more_contacts),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.TERMS_AND_CONDITIONS,
                    itemText = StringSource(R.string.tab_more_terms_and_conditions),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.ADMINISTRATORS,
                    itemText = StringSource(R.string.tab_more_administrators),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.CENTERS_CERTIFICATION_SERVICES,
                    itemText = StringSource(R.string.tab_more_centers_certification_services),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.PROVIDERS_ELECTRONIC_ADMINISTRATIVE_SERVICES,
                    itemText = StringSource(R.string.tab_more_providers_electronic_administrative_services),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.ELECTRONIC_DELIVERY_SYSTEM,
                    itemText = StringSource(R.string.tab_more_electronic_delivery_system),
                )
            )
            add(
                TabMoreItemUi(
                    type = TabMoreItems.ONLINE_HELP_SYSTEM,
                    itemText = StringSource(R.string.tab_more_online_help_system),
                )
            )
        }
    }

}