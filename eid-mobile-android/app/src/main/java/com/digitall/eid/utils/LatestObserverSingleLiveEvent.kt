package com.digitall.eid.utils

import androidx.annotation.MainThread
import androidx.lifecycle.Lifecycle
import androidx.lifecycle.LifecycleEventObserver
import androidx.lifecycle.LifecycleOwner
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.Observer
import java.util.concurrent.atomic.AtomicBoolean

/**
 * A LiveData that delivers an event only once to the most recently registered
 * active observer. If multiple observers are active, only the one that
 * called `observe()` most recently will receive the event.
 *
 * Note: This behavior is different from typical SingleLiveEvent which delivers
 * to one active observer without specific preference for "latest".
 * This version is more specialized for scenarios where such "latest priority" is crucial.
 */
class LatestObserverSingleLiveEvent<T>() : MutableLiveData<T>() {

    private val pending = AtomicBoolean(false)

    private var latestObserverWrapper: ObserverWrapper? = null
    private val activeObserverWrappers = mutableMapOf<Observer<in T>, ObserverWrapper>()

    constructor(value: T) : this() {
        setValue(value)
    }

    @MainThread
    override fun observe(owner: LifecycleOwner, observer: Observer<in T>) {
        activeObserverWrappers.values.find { it.owner == owner }?.let { existingWrapper ->
            super.removeObserver(existingWrapper.internalObserver)
            activeObserverWrappers.remove(existingWrapper.userObserver)
        }

        val wrapper = ObserverWrapper(owner, observer)
        activeObserverWrappers[observer] = wrapper
        latestObserverWrapper = wrapper

        owner.lifecycle.addObserver(object : LifecycleEventObserver {
            override fun onStateChanged(source: LifecycleOwner, event: Lifecycle.Event) {
                if (event == Lifecycle.Event.ON_DESTROY) {
                    removeObserver(observer) // This will call our overridden removeObserver
                    source.lifecycle.removeObserver(this)
                }
            }
        })

        super.observe(owner, wrapper.internalObserver)
    }

    @MainThread
    override fun removeObserver(observer: Observer<in T>) {
        val wrapperToRemove = activeObserverWrappers.remove(observer)
        wrapperToRemove?.let {
            super.removeObserver(it.internalObserver)
            if (latestObserverWrapper == it) {

                latestObserverWrapper = activeObserverWrappers.values.maxByOrNull {
                    0
                }
            }
        } ?: run {
            try {
                super.removeObserver(observer)
            } catch (_: Exception) {
            }
        }
    }


    @MainThread
    override fun setValue(t: T?) {
        pending.set(true)
        super.setValue(t) // This will trigger all active internal observers
    }

    /**
     * A convenience method for events that don't carry data (Void).
     */
    @MainThread
    fun call() {
        value = null
    }

    private inner class ObserverWrapper(
        val owner: LifecycleOwner,
        val userObserver: Observer<in T> // The observer provided by the user
    ) {
        // The internal observer that we register with LiveData
        val internalObserver = Observer<T> { value ->
            // Only proceed if this wrapper is the designated "latest"
            // and the event is pending
            if (this == latestObserverWrapper && pending.compareAndSet(true, false)) {
                userObserver.onChanged(value)
            }
        }
    }
}