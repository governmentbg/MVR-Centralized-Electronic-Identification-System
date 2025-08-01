package com.digitall.eid.domain.extensions


inline fun <reified T> List<*>.isOfType(): Boolean {
    if (this.isEmpty()) {
        // Conventionally, an empty list can be considered a list of any type.
        // You might adjust this behavior based on your specific needs.
        return true
    }
    return this.all { it is T }
}