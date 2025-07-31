package com.digitall.eid.ui.view

import android.app.Dialog
import android.content.Context
import android.os.Bundle
import androidx.appcompat.widget.LinearLayoutCompat
import com.digitall.eid.R
import com.digitall.eid.databinding.LayoutFullscreenLoaderBinding
import com.digitall.eid.models.common.StringSource

class FullscreenLoaderView(context: Context): Dialog(context)  {

    private val binding = LayoutFullscreenLoaderBinding.inflate(layoutInflater)

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(binding.root)
        setBackground()
        setCancelable(false)
        setCanceledOnTouchOutside(false)
    }

    fun setMessage(message: StringSource) {
        binding.tvMessage.text = message.getString(context)
    }

    private fun setBackground() {
        window?.setLayout(
            LinearLayoutCompat.LayoutParams.MATCH_PARENT,
            LinearLayoutCompat.LayoutParams.MATCH_PARENT
        )
        window?.setBackgroundDrawableResource(R.color.color_EFF6FF)
    }

}