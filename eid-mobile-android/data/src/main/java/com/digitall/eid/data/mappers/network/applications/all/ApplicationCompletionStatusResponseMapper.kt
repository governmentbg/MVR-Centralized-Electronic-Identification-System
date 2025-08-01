package com.digitall.eid.data.mappers.network.applications.all

import com.digitall.eid.data.mappers.base.BaseMapper
import com.digitall.eid.domain.extensions.getEnumValue
import com.digitall.eid.domain.models.applications.all.ApplicationCompletionStatusEnum

class ApplicationCompletionStatusResponseMapper :
    BaseMapper<String, ApplicationCompletionStatusEnum>() {

    override fun map(from: String): ApplicationCompletionStatusEnum {
        return getEnumValue<ApplicationCompletionStatusEnum>(from)
            ?: ApplicationCompletionStatusEnum.UNKNOWN
    }

}