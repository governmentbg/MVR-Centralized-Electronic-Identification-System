/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.empowerment.signing

import android.os.Parcelable
import com.digitall.eid.R
import com.digitall.eid.domain.models.base.TypeEnum
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonListElementIdentifier
import kotlinx.parcelize.Parcelize

@Parcelize
enum class EmpowermentFromMeSigningEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    SPINNER_METHOD("SPINNER_METHOD", StringSource(R.string.signing_type)),
    BUTTON_SIGN("BUTTON_SIGN", StringSource(R.string.sign)),
    BUTTON_BACK("BUTTON_BACK", StringSource(R.string.back)),
}

@Parcelize
enum class EmpowermentFromMeSigningMethodsEnumUi(
    override val type: String,
    val title: StringSource,
) : CommonListElementIdentifier, TypeEnum, Parcelable {
    EVROTRUST("EVROTRUST", StringSource(R.string.signing_method_evrotrust_enum_type)),
    BORIKA("BORIKA", StringSource(R.string.signing_method_borica_enum_type)),
}