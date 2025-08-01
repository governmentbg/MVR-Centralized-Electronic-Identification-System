/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import androidx.annotation.DrawableRes
import androidx.annotation.StringRes
import com.digitall.eid.R

data class BannerMessage(
    val messageID: String? = null,
    val title: StringSource? = null,
    val message: StringSource,
    @param:DrawableRes val icon: Int? = R.drawable.ic_error,
    val state: State = State.ERROR,
    val gravity: Gravity = Gravity.START,
) {

    enum class State {
        ERROR,
        SUCCESS
    }

    enum class Gravity {
        START,
        CENTER
    }

    companion object {
        /**
         * Red error banners with [message] and error [icon] and gravity start.
         */
        fun error(
            message: StringSource,
            @DrawableRes icon: Int = R.drawable.ic_error
        ): BannerMessage {
            return BannerMessage(
                message = message,
                icon = icon,
                state = State.ERROR
            )
        }

        /**
         * Green success banners with [message] and success [icon] and gravity start.
         */
        fun success(
            message: String,
            @DrawableRes icon: Int = R.drawable.ic_success
        ): BannerMessage {
            return BannerMessage(
                message = StringSource(message),
                icon = icon,
                state = State.SUCCESS
            )
        }

        fun success(
            @StringRes message: Int,
            @DrawableRes icon: Int = R.drawable.ic_success
        ): BannerMessage {
            return BannerMessage(
                message = StringSource(message),
                icon = icon,
                state = State.SUCCESS
            )
        }

        /**
         * Green success banners with simple [message] in the center of the banner.
         */
        fun successCenter(message: String): BannerMessage {
            return BannerMessage(
                message = StringSource(message),
                icon = null,
                state = State.SUCCESS,
                gravity = Gravity.CENTER
            )
        }

        fun successCenter(@StringRes message: Int): BannerMessage {
            return BannerMessage(
                message = StringSource(message),
                icon = null,
                state = State.SUCCESS,
                gravity = Gravity.CENTER
            )
        }
    }
}