package com.digitall.eid.domain.models.applications.create

import com.digitall.eid.domain.models.administrators.AdministratorModel

data class ApplicationCreateInitialInformationModel(
    val userModel: ApplicationUserDetailsModel? = null ,
    val administrators: List<AdministratorModel>? = null
) {
    val isValid: Boolean
        get() = userModel != null && administrators != null
}