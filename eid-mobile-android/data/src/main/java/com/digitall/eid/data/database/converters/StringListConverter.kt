/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database.converters

import androidx.room.TypeConverter

class StringListConverter {

    @TypeConverter
    fun toString(list: MutableList<String>): String {
        return StringBuilder().run {
            list.forEachIndexed { index, s ->
                append(s)
                if (index + 1 < list.size) {
                    append(SEPARATOR)
                }
            }
            toString()
        }
    }

    companion object {
        private const val SEPARATOR = "###"
    }

}