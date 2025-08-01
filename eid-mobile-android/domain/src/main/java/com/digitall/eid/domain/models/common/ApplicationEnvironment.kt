package com.digitall.eid.domain.models.common

import com.digitall.eid.domain.models.base.TypeEnum

enum class ApplicationEnvironment(
    override val type: String,
    val urlBase: String,
    val urlPopop: String,
    val urlKeycloakPg: String,
    val urlPan: String,
    val urlRo: String,
    val urlSigning: String,
    val urlPjs: String,
    val urlMpozei: String,
    val urlIsceigw: String,
    val urlRaeicei: String,
    val urlPayment: String,
): TypeEnum {
    DIGITALL_DEV(
        type = "",
        urlBase = "",
        urlPopop = "",
        urlKeycloakPg = "",
        urlPan = "",
        urlRo = "",
        urlSigning = "",
        urlPjs = "",
        urlMpozei = "",
        urlIsceigw = "",
        urlRaeicei = "",
        urlPayment = ""
    ),
}
