package com.digitall.eid.data.extensions

import org.json.JSONArray
import org.json.JSONException
import org.json.JSONObject

fun String.toCamelCase(): String {
    val pattern = "_[a-z]".toRegex()
    return lowercase().replace(pattern) { it.value.last().uppercase() }
}

fun String.isJSONValid(): Boolean {
    try {
        JSONObject(this)
    } catch (_: JSONException) {
        try {
            JSONArray(this)
        } catch (_: JSONException) {
            return false
        }
    }
    return true
}