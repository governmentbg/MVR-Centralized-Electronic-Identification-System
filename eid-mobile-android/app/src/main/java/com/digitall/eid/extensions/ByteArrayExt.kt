package com.digitall.eid.extensions

fun ByteArray.toHex() = joinToString("") { "%02x".format(it) }