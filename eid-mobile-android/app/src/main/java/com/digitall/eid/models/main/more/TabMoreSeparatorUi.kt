package com.digitall.eid.models.main.more

import com.digitall.eid.extensions.equalTo

class TabMoreSeparatorUi(
    val marginLeft: Int = 0,
    val marginTop: Int = 0,
    val marginRight: Int = 0,
    val marginBottom: Int = 0,
) : TabMoreAdapterMarker {

    override fun isItemSame(other: Any?): Boolean {
        return equalTo(other)
    }

    override fun isContentSame(other: Any?): Boolean {
        return equalTo(
            other,
            { marginLeft },
            { marginTop },
            { marginRight },
            { marginBottom },
        )
    }

}