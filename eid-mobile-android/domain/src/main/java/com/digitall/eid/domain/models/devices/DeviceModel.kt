package com.digitall.eid.domain.models.devices

import com.digitall.eid.domain.models.common.OriginalModel
import kotlinx.parcelize.Parcelize

@Parcelize
data class DeviceModel(
    val id: String?,
    val name: String?,
    val type: String?,
    val description: String?,
): OriginalModel
