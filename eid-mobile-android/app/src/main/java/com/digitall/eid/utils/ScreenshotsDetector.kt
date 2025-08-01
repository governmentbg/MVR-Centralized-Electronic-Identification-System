/**
 * This class is working only when the read storage permission is granted for reading
 * image files from Screenshot external directory. Otherwise doing nothing.
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 */
package com.digitall.eid.utils

import android.app.Activity
import java.io.File

interface ScreenshotsDetector {

    /**
     * Has to be called in either onStart or onResume. The [listener] returns the screenshot
     * if the permission is granted and all necessary requirements are met.
     */

    fun startDetecting(activity: Activity, listener: (File?) -> Unit)

    /**
     * Has to be called in either onStop or onPause to unsubscribe
     * the listener from the activity.
     */

    fun stopDetecting(activity: Activity)

}