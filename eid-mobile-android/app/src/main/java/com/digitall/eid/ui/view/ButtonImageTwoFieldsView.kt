/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.util.AttributeSet
import android.view.LayoutInflater
import android.view.View
import androidx.appcompat.widget.LinearLayoutCompat
import androidx.core.content.withStyledAttributes
import com.digitall.eid.R
import com.digitall.eid.databinding.LayoutButtonImageTwoFieldsBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.setBackgroundDrawableResource

class ButtonImageTwoFieldsView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : LinearLayoutCompat(context, attrs, defStyleAttr) {

    companion object {
        private const val TAG = "CustomToolbarTag"
    }

    private val binding =
        LayoutButtonImageTwoFieldsBinding.inflate(LayoutInflater.from(context), this)

    var clickListener: (() -> Unit)? = null

    init {
        setupView()
        setupAttributes(attrs)
        setupControls()
    }

    private fun setupView() {
        orientation = HORIZONTAL
        setBackgroundDrawableResource(R.drawable.bg_field_with_corners)
    }

    private fun setupAttributes(attrs: AttributeSet?) {
        context.withStyledAttributes(attrs, R.styleable.ButtonImageTwoFields) {
            val title = getString(R.styleable.ButtonImageTwoFields_button_image_two_fields_title)
            if (!title.isNullOrEmpty()) {
                binding.tvTitle.visibility = View.VISIBLE
                binding.tvTitle.text = title
            } else {
                binding.tvTitle.visibility = View.GONE
            }
            val description =
                getString(R.styleable.ButtonImageTwoFields_button_image_two_fields_description)
            if (!description.isNullOrEmpty()) {
                binding.tvDescription.visibility = View.VISIBLE
                binding.tvDescription.text = description
            } else {
                binding.tvDescription.visibility = View.GONE
            }
            val icon =
                getDrawable(R.styleable.ButtonImageTwoFields_button_image_two_fields_icon)
            if (icon != null) {
                binding.ivIcon.visibility = View.VISIBLE
                binding.ivIcon.setImageDrawable(icon)
            } else {
                binding.ivIcon.visibility = View.GONE
            }
        }
    }

    private fun setupControls() {
        onClickThrottle {
            clickListener?.invoke()
        }
    }


}