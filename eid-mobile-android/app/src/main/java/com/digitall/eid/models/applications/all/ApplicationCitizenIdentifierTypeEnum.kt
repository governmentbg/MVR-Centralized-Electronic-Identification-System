package com.digitall.eid.models.applications.all

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class ApplicationCitizenIdentifierTypeEnum(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum {
    EGN("EGN", StringSource(R.string.egn)),
    LNCH("LNCH", StringSource(R.string.lnch)),
}