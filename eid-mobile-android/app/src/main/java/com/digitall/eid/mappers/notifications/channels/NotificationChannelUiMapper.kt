/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.mappers.notifications.channels

import com.digitall.eid.R
import com.digitall.eid.data.mappers.base.BaseMapperWithData
import com.digitall.eid.domain.models.common.ApplicationLanguage
import com.digitall.eid.domain.models.notifications.channels.NotificationsChannelsModel
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.notifications.channels.NotificationChannelUi

class NotificationChannelUiMapper :
    BaseMapperWithData<NotificationsChannelsModel, ApplicationLanguage, List<NotificationChannelUi>>() {

    override fun map(
        from: NotificationsChannelsModel,
        data: ApplicationLanguage?
    ): List<NotificationChannelUi> {
        return buildList {
            from.channels?.forEach { channel ->
                add(
                    NotificationChannelUi(
                        id = channel.id,
                        name = channel.translations?.firstOrNull { translation -> translation.language == data?.type }
                            ?.let { translation ->
                                StringSource(translation.name)
                            } ?: StringSource(R.string.unknown),
                        isEnabled = channel.isEnabled == true,
                        isSelected = from.enabledChannels?.contains(channel.id) == true,
                        description = channel.translations?.firstOrNull { translation -> translation.language == data?.type }
                            ?.let { translation ->
                                StringSource(translation.description)
                            } ?: StringSource(R.string.unknown)
                    )
                )
            }
        }
    }
}