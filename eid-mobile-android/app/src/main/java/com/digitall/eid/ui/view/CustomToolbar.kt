/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.ui.view

import android.content.Context
import android.graphics.Color
import android.util.AttributeSet
import android.view.LayoutInflater
import android.view.View
import androidx.annotation.ColorRes
import androidx.annotation.DrawableRes
import androidx.constraintlayout.widget.ConstraintLayout
import androidx.constraintlayout.widget.ConstraintSet
import androidx.core.content.withStyledAttributes
import androidx.core.view.isVisible
import com.digitall.eid.R
import com.digitall.eid.databinding.LayoutToolbarBinding
import com.digitall.eid.extensions.onClickThrottle
import com.digitall.eid.extensions.pxDimen
import com.digitall.eid.extensions.setTextSource
import com.digitall.eid.extensions.showSpinner
import com.digitall.eid.extensions.tintColor
import com.digitall.eid.extensions.tintRes
import com.digitall.eid.models.common.StringSource
import com.digitall.eid.models.list.CommonSpinnerMenuItemUi
import com.digitall.eid.models.list.CommonSpinnerUi

class CustomToolbar @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : ConstraintLayout(context, attrs, defStyleAttr) {

    companion object {
        private const val TAG = "CustomToolbarTag"
    }

    private val binding = LayoutToolbarBinding.inflate(LayoutInflater.from(context), this, true)

    var navigationClickListener: (() -> Unit)? = null

    var settingsClickListener: (() -> Unit)? = null


    init {
        setupAttributes(attrs)
        setupControls()
    }

    override fun onMeasure(widthMeasureSpec: Int, heightMeasureSpec: Int) {
        val height = context.pxDimen(R.dimen.toolbar_height)
        super.onMeasure(
            widthMeasureSpec,
            MeasureSpec.makeMeasureSpec(height, MeasureSpec.EXACTLY)
        )
    }

    private fun setupAttributes(attrs: AttributeSet?) {
        context.withStyledAttributes(attrs, R.styleable.CustomToolbar) {
            // Title
            val titleText = getString(R.styleable.CustomToolbar_toolbar_title)
            setTitleText(titleText)

            val titleTextColor = getColor(
                R.styleable.CustomToolbar_toolbar_elements_color,
                Color.WHITE // Default color if not provided
            )
            setTitleTextColor(titleTextColor)


            // Background
            val backgroundColor = getColor(
                R.styleable.CustomToolbar_toolbar_background_color,
                Color.WHITE// Default from your colors
            )
            binding.toolbarRoot.setBackgroundColor(backgroundColor)


            // Navigation Icon
            val navIconResId = getResourceId(R.styleable.CustomToolbar_toolbar_navigation_icon, 0)
            if (navIconResId != 0) {
                setNavigationIcon(navIconResId)
            } else {
                hideNavigationIcon() // Ensure title is adjusted if no icon initially
            }


            // Settings Icon
            val settingsIconResId =
                getResourceId(R.styleable.CustomToolbar_toolbar_settings_icon, 0)
            if (settingsIconResId != 0) {
                setSettingsIcon(settingsIconResId)
            } else {
                hideSettingsIcon()
            }

            val elementsColor = getColor(
                R.styleable.CustomToolbar_toolbar_elements_color,
                Color.WHITE // Default fallback
            )
            setElementsColor(elementsColor)

            updateTitleConstraints()
        }
    }

    private fun setupControls() {
        binding.icNavigation.onClickThrottle {
            navigationClickListener?.invoke()
        }
        binding.icSettings.onClickThrottle {
            settingsClickListener?.invoke()
        }
    }

    private fun setElementsColor(color: Int) {
        binding.icNavigation.tintColor(color)
        binding.tvTitle.setTextColor(color)
        binding.icSettings.tintColor(color)
    }

    fun setTitleText(title: String?) {
        if (title.isNullOrEmpty()) {
            binding.tvTitle.visibility = View.GONE
        } else {
            binding.tvTitle.text = title
            binding.tvTitle.visibility = View.VISIBLE
        }
    }

    private fun setTitleTextColor(color: Int) {
        binding.tvTitle.setTextColor(color)
    }

    private fun setNavigationIcon(iconResId: Int) {
        if (iconResId != 0) {
            binding.icNavigation.setImageResource(iconResId)
            binding.icNavigation.visibility = View.VISIBLE
        } else {
            binding.icNavigation.visibility = View.GONE
        }
        updateTitleConstraints() // Update constraints when nav icon visibility changes
    }

    private fun hideNavigationIcon() {
        binding.icNavigation.visibility = View.GONE
        navigationClickListener = null
        updateTitleConstraints() // Update constraints
    }

    private fun setSettingsIcon(@DrawableRes iconResId: Int) {
        if (iconResId != 0) {
            binding.icSettings.setImageResource(iconResId)
            binding.icSettings.visibility = View.VISIBLE
        } else {
            binding.icSettings.visibility = View.GONE
        }
    }

    private fun hideSettingsIcon() {
        binding.icSettings.visibility = View.GONE
        settingsClickListener = null
        updateTitleConstraints()
    }

    fun setTitleText(text: StringSource) {
        binding.tvTitle.setTextSource(text)
        binding.tvTitle.visibility = View.VISIBLE
    }

    fun setSettingsIcon(
        @DrawableRes settingsIconRes: Int = R.drawable.ic_arrow_down,
        @ColorRes settingsIconColorRes: Int = R.color.color_white,
        settingsClickListener: (() -> Unit),
    ) {
        binding.icSettings.setImageResource(settingsIconRes)
        binding.icSettings.tintRes(settingsIconColorRes)
        binding.icSettings.visibility = View.VISIBLE
        this.settingsClickListener = settingsClickListener
        updateTitleConstraints()
    }

    fun setNavigationIcon(
        @DrawableRes navigationIconRes: Int = R.drawable.ic_arrow_left,
        @ColorRes navigationIconColorRes: Int = R.color.color_white,
        navigationClickListener: (() -> Unit),
    ) {
        binding.icNavigation.setImageResource(navigationIconRes)
        binding.icNavigation.tintRes(navigationIconColorRes)
        binding.icNavigation.visibility = View.VISIBLE
        this.navigationClickListener = navigationClickListener
        updateTitleConstraints()
    }

    fun showSettingsSpinner(
        model: CommonSpinnerUi,
        clickListener: ((model: CommonSpinnerMenuItemUi) -> Unit)
    ) {
        binding.icSettings.showSpinner(
            model = model,
            clickListener = clickListener,
        )
    }

    fun requestBarrierUpdate() {
        binding.barrierSettings.requestLayout()
    }

    private fun updateTitleConstraints() {
        val constraintSet = ConstraintSet()
        constraintSet.clone(binding.toolbarRoot) // Assuming binding.toolbarRoot is your ConstraintLayout

        val titleId = binding.tvTitle.id
        val navIconId = binding.icNavigation.id
        val barrierSettingsId = binding.barrierSettings.id
        val parentId = ConstraintSet.PARENT_ID

        // Margin when title is next to an icon (either nav or settings)
        val titleAdjacentIconMargin =
            resources.getDimensionPixelSize(R.dimen.toolbar_title_icon_margin)

        // Padding when title is next to a parent edge because the icon on that side is missing
        // Use this when ONLY ONE side has an icon, and the other side is parent.
        val titleParentEdgePaddingOppositeIcon =
            resources.getDimensionPixelSize(R.dimen.toolbar_title_parent_edge_padding_when_opposite_icon_exists)

        // Padding when title is alone (no nav, no settings)
        val titleParentEdgePaddingLone =
            resources.getDimensionPixelSize(R.dimen.toolbar_title_parent_edge_padding_lone)

        // Clear existing horizontal constraints on title before reconnecting to be safe
        constraintSet.clear(titleId, ConstraintSet.START)
        constraintSet.clear(titleId, ConstraintSet.END)

        val isNavVisible = binding.icNavigation.isVisible
        val areSettingsVisible = binding.icSettings.isVisible

        // --- Determine Start Constraint & Margin ---
        if (isNavVisible) {
            constraintSet.connect(
                titleId,
                ConstraintSet.START,
                navIconId,
                ConstraintSet.END,
                titleAdjacentIconMargin
            )
        } else {
            // Nav is hidden. Connect to parent start.
            // If settings are also hidden (title is lone), use titleParentEdgePaddingLone.
            // If settings are visible, use titleParentEdgePaddingOppositeIcon.
            val actualStartPadding =
                if (areSettingsVisible) titleParentEdgePaddingOppositeIcon else titleParentEdgePaddingLone
            constraintSet.connect(
                titleId,
                ConstraintSet.START,
                parentId,
                ConstraintSet.START,
                actualStartPadding
            )
        }

        // --- Determine End Constraint & Margin ---
        if (areSettingsVisible) {
            constraintSet.connect(
                titleId,
                ConstraintSet.END,
                barrierSettingsId,
                ConstraintSet.START,
                titleAdjacentIconMargin
            )
        } else {
            // Settings are hidden. Connect to parent end.
            // If nav is also hidden (title is lone), use titleParentEdgePaddingLone.
            // If nav is visible, use titleParentEdgePaddingOppositeIcon FOR SYMMETRY with startMargin.
            val actualEndPadding =
                if (isNavVisible) titleParentEdgePaddingOppositeIcon else titleParentEdgePaddingLone
            // If you want the margin to parent_end to be identical to the margin from nav_icon:
            // val actualEndPadding = if (isNavVisible) startMargin /* which is titleAdjacentIconMargin */ else titleParentEdgePaddingLone;
            constraintSet.connect(
                titleId,
                ConstraintSet.END,
                parentId,
                ConstraintSet.END,
                actualEndPadding
            )
        }

        // Ensure horizontal bias is set (if not already correctly in XML, or to be certain)
        constraintSet.setHorizontalBias(titleId, 0.5f)

        // Ensure title is vertically centered (this centers the TextView block)
        constraintSet.centerVertically(titleId, parentId)

        constraintSet.applyTo(binding.toolbarRoot)
    }
}