package com.digitall.eid.extensions

fun String.noLeadingTrailingSpaces() = replace("(\\s|\\n)+".toRegex(), " ").trim()
fun String.appendIfMissing(suffix: String) = if (endsWith(suffix)) this else (this + suffix)
fun String.prependIfMissing(prefix: String) = if (startsWith(prefix)) this else (prefix + this)
fun String.toByteArray() = chunked(2).map { it.toInt(16).toByte() }.toByteArray()