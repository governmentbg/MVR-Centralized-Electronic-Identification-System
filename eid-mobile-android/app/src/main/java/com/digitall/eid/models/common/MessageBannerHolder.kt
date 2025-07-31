/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.models.common

import android.view.View

interface MessageBannerHolder {

    fun showMessage(message: BannerMessage, anchorView: View? = null)

    fun showMessage(message: DialogMessage)

    fun showFullscreenLoader(message: StringSource?)

    fun hideFullscreenLoader()

}