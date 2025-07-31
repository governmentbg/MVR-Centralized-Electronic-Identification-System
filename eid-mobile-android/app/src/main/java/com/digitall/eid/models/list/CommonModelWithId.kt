/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2024 by Roman Kryvolapov
 **/
package com.digitall.eid.models.list

import android.os.Parcelable

interface CommonModelWithId : Parcelable {
    val elementId: Int?
    val elementEnum: CommonListElementIdentifier?
}