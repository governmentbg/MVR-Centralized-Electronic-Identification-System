package com.digitall.eid.models.empowerment.create.create

import com.digitall.eid.domain.models.empowerment.common.all.EmpowermentItem
import com.digitall.eid.domain.models.user.UserModel

data class EmpowermentCreateUiModel(
    val user: UserModel? = null,
    val empowermentItem: EmpowermentItem? = null,
)
