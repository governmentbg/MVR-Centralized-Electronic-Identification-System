/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.util.AttributeSet
import android.view.LayoutInflater
import android.widget.FrameLayout
import androidx.core.content.withStyledAttributes
import com.digitall.eid.R
import com.digitall.eid.databinding.LayoutLoaderBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.setBackgroundColorResource

class LoaderView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : FrameLayout(context, attrs, defStyleAttr) {

    companion object {
        private const val TAG = "LoaderViewTag"
    }

    private val binding = LayoutLoaderBinding.inflate(LayoutInflater.from(context), this)

    init {
        setBackgroundColorResource(R.color.color_transparent)
        setupAttributes(attrs)
        isClickable = true
        isFocusable = true
    }

    private fun setupAttributes(attrs: AttributeSet?) {
        context.withStyledAttributes(attrs, R.styleable.LoaderView) {
            getString(R.styleable.LoaderView_loader_view_message)?.let {
                logDebug("tvMessage text: $it", TAG)
                binding.tvMessage.text = it
            }
        }
    }

    fun setMessage(message: String) {
        binding.tvMessage.text = message
    }

}