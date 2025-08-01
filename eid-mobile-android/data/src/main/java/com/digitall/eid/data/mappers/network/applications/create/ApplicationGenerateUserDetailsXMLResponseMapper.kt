/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.data.mappers.network.applications.create

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.data.models.network.applications.create.ApplicationGenerateUserDetailsXMLResponse
import com.digitall.eid.domain.models.applications.create.ApplicationGenerateUserDetailsXMLResponseModel

class ApplicationGenerateUserDetailsXMLResponseMapper :
    BaseMapper<ApplicationGenerateUserDetailsXMLResponse, ApplicationGenerateUserDetailsXMLResponseModel>() {

    override fun map(from: ApplicationGenerateUserDetailsXMLResponse): ApplicationGenerateUserDetailsXMLResponseModel {
        return with(from) {
            ApplicationGenerateUserDetailsXMLResponseModel(
                xml = xml,
            )
        }
    }

}