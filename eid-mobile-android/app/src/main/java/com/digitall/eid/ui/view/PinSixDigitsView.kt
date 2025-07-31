/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.text.Editable
import android.util.AttributeSet
import android.view.KeyEvent
import android.view.LayoutInflater
import android.view.View.OnKeyListener
import androidx.appcompat.widget.AppCompatEditText
import androidx.appcompat.widget.LinearLayoutCompat
import androidx.core.widget.addTextChangedListener
import com.digitall.eid.R
import com.digitall.eid.databinding.LayoutPinBinding
import com.digitall.eid.domain.utils.LogUtil.logDebug
import com.digitall.eid.extensions.onDonePressed
import com.digitall.eid.extensions.setBackgroundColorResource
import kotlin.math.abs

class PinSixDigitsView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : LinearLayoutCompat(context, attrs, defStyleAttr) {

    companion object {
        private const val TAG = "PinSixDigitsViewTag"
    }

    private val binding = LayoutPinBinding.inflate(LayoutInflater.from(context), this)

    var onPinStartEnter: (() -> Unit)? = null

    var onPinEntered: ((String) -> Unit)? = null

    var onPinCleared: (() -> Unit)? = null

    private var pin: Array<Int?> = arrayOf(null, null, null, null, null, null)

    private lateinit var numInputFields: List<AppCompatEditText>

    init {
        setupView()
        setupControls()
    }

    private fun setupView() {
        orientation = HORIZONTAL
        setBackgroundColorResource(R.color.color_white)
        numInputFields = listOf(
            binding.num1,
            binding.num2,
            binding.num3,
            binding.num4,
            binding.num5,
            binding.num6
        )
    }

    private fun setupControls() {
        numInputFields.forEachIndexed { index, appCompatEditText ->
            appCompatEditText.setOnClickListener {
                onPinEntered?.invoke(pin.filterNotNull().joinToString(""))
            }
            appCompatEditText.setOnKeyListener(OnKeyListener { _, keyCode, event ->
                return@OnKeyListener when {
                    keyCode == KeyEvent.KEYCODE_DEL && event.action == KeyEvent.ACTION_DOWN -> {
                        clearPin()
                        true
                    }

                    keyCode == KeyEvent.KEYCODE_ENTER && event.action == KeyEvent.ACTION_DOWN -> {
                        onPinEntered?.invoke(pin.filterNotNull().joinToString(""))
                        true
                    }

                    else -> false
                }
            })
            appCompatEditText.setOnFocusChangeListener { view, _ ->
                val firstEmptyIndex =
                    numInputFields.indexOfFirst { field -> field.text.isNullOrEmpty() }
                val currentIndex = numInputFields.indexOf(view)

                if (firstEmptyIndex != -1 && abs(currentIndex - firstEmptyIndex) >= 1) {
                    view.clearFocus()
                    numInputFields[firstEmptyIndex].requestFocus()
                }

            }
            appCompatEditText.addTextChangedListener(afterTextChanged = { text: Editable? ->
                if (text.isNullOrEmpty()) {
                    pin = Array(size = 6, init = { null })
                    onPinCleared?.invoke()
                    return@addTextChangedListener
                }

                onPinStartEnter?.invoke()
                pin[index] = text.toString().toInt()
                postDelayed(
                    {
                        when {
                            index < numInputFields.size - 1 -> numInputFields[index + 1].requestFocus()
                            else -> {
                                appCompatEditText.clearFocus()
                                onPinEntered?.invoke(pin.filterNotNull().joinToString(""))
                            }
                        }
                    },
                    50
                )
            })
            appCompatEditText.onDonePressed {}
        }
    }

    fun clearPin() {
        logDebug("clearPin", TAG)
        numInputFields.forEach {
            it.text?.clear()
        }
        pin = Array(size = 6, init = { null })
        numInputFields.first().requestFocus()
    }

}