package com.digitall.eid.models.common

import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum

enum class ApplicationEnvironmentUi(override val type: String, val title: StringSource) : TypeEnum {
    DIGITALL_DEV(type = "DIGITALL_DEV", title = StringSource(R.string.environment_digitall_dev)),
    MVR_DEV(type = "MVR_DEV", title = StringSource(R.string.environment_mvr_dev)),
    MVR_TEST(type = "MVR_TEST", title = StringSource(R.string.environment_mvr_test)),
    MVR_STAGE(type = "MVR_STAGE", title = StringSource(R.string.environment_mvr_stage)),
}