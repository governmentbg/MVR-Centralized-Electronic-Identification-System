package com.digitall.eid.ui.common.list

import android.graphics.Rect
import android.text.SpannableStringBuilder
import android.text.Spanned
import android.text.style.RelativeSizeSpan
import android.view.ViewGroup
import androidx.core.content.ContextCompat
import androidx.core.graphics.drawable.DrawableCompat
import androidx.core.graphics.drawable.toBitmap
import androidx.recyclerview.widget.RecyclerView
import com.digitall.eid.databinding.ListItemSimpleTextBinding
import com.digitall.eid.extensions.inflateBinding
import com.digitall.eid.models.list.CommonLabeledSimpleTextUi
import com.digitall.eid.models.list.CommonListElementAdapterMarker
import com.digitall.eid.ui.view.CenteredImageSpan
import com.digitall.eid.utils.RoundedBackgroundSpan
import com.hannesdorfmann.adapterdelegates4.AdapterDelegate
import org.koin.core.component.KoinComponent

class CommonLabeledSimpleTextDelegate :
    AdapterDelegate<MutableList<CommonListElementAdapterMarker>>(), KoinComponent {

    companion object {
        private const val TAG = "CommonLabeledSimpleTextDelegateTag"
    }

    override fun isForViewType(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int
    ): Boolean {
        return items[position] is CommonLabeledSimpleTextUi
    }

    override fun onCreateViewHolder(parent: ViewGroup): RecyclerView.ViewHolder {
        return ViewHolder(parent.inflateBinding(ListItemSimpleTextBinding::inflate))
    }

    override fun onBindViewHolder(
        items: MutableList<CommonListElementAdapterMarker>,
        position: Int,
        holder: RecyclerView.ViewHolder,
        payloads: MutableList<Any>
    ) {
        (holder as ViewHolder).bind(items[position] as CommonLabeledSimpleTextUi)
    }

    private inner class ViewHolder(
        private val binding: ListItemSimpleTextBinding
    ) : RecyclerView.ViewHolder(binding.root) {

        fun bind(model: CommonLabeledSimpleTextUi) {
            binding.tvTitle.text = model.title.getString(binding.root.context)
            val labeledText = model.labeledText.getString(binding.root.context)
            val text = model.text.getString(binding.root.context)
            val iconBounds = Rect(-2, -2, 46, 46)
            binding.tvValue.text = when {
                model.iconResLeft != null -> {
                    ContextCompat.getDrawable(binding.root.context, model.iconResLeft)
                        ?.let { drawable ->
                            val wrappedDrawable = DrawableCompat.wrap(drawable)
                            val color = ContextCompat.getColor(
                                binding.root.context,
                                model.labelTextColorRes
                            )
                            DrawableCompat.setTint(wrappedDrawable, color)
                            drawable.bounds = iconBounds
                            SpannableStringBuilder().apply {
                                append(" ", CenteredImageSpan(drawable), 0)
                                append(" ")
                                append("$labeledText \n")
                                setSpan(
                                    RoundedBackgroundSpan(
                                        cornerRadius = 4,
                                        backgroundColor = ContextCompat.getColor(
                                            binding.root.context,
                                            model.labelBackgroundColorRes
                                        ),
                                        textColor = ContextCompat.getColor(
                                            binding.root.context,
                                            model.labelTextColorRes
                                        ),
                                        leftBitmap = drawable.toBitmap(width = 32, height = 32)
                                    ),
                                    0,
                                    length,
                                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                                )
                                append(text)
                                setSpan(
                                    RelativeSizeSpan(
                                        .8f
                                    ),
                                    length - text.length,
                                    length,
                                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                                )
                            }
                        }
                }

                model.iconResRight != null -> {
                    ContextCompat.getDrawable(binding.root.context, model.iconResRight)
                        ?.let { drawable ->
                            val wrappedDrawable = DrawableCompat.wrap(drawable)
                            val color = ContextCompat.getColor(
                                binding.root.context,
                                model.labelTextColorRes
                            )
                            DrawableCompat.setTint(wrappedDrawable, color)
                            drawable.bounds = iconBounds
                            SpannableStringBuilder().apply {
                                append(labeledText)
                                append(" ")
                                append(" ", CenteredImageSpan(drawable), 0)
                                append("\n")
                                setSpan(
                                    RoundedBackgroundSpan(
                                        cornerRadius = 4,
                                        backgroundColor = ContextCompat.getColor(
                                            binding.root.context,
                                            model.labelBackgroundColorRes
                                        ),
                                        textColor = ContextCompat.getColor(
                                            binding.root.context,
                                            model.labelTextColorRes
                                        ),
                                        rightBitmap = drawable.toBitmap(width = 32, height = 32)
                                    ),
                                    0,
                                    length,
                                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                                )
                                append(text)
                                setSpan(
                                    RelativeSizeSpan(
                                        .8f
                                    ),
                                    length - text.length,
                                    length,
                                    Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                                )
                            }
                        }
                }

                else -> {
                    SpannableStringBuilder().apply {
                        append("$labeledText \n")
                        setSpan(
                            RoundedBackgroundSpan(
                                cornerRadius = 4,
                                backgroundColor = ContextCompat.getColor(
                                    binding.root.context,
                                    model.labelBackgroundColorRes
                                ),
                                textColor = ContextCompat.getColor(
                                    binding.root.context,
                                    model.labelTextColorRes
                                )
                            ),
                            0,
                            length,
                            Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                        )
                        append(text)
                        setSpan(
                            RelativeSizeSpan(
                                .8f
                            ),
                            length - text.length,
                            length,
                            Spanned.SPAN_EXCLUSIVE_EXCLUSIVE
                        )
                    }
                }
            }
        }
    }
}