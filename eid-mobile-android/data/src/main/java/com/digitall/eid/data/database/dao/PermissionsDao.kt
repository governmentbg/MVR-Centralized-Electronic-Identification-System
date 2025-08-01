/**
 * Please follow code style when editing project
 * Please follow principles of clean architecture
 * Created 2023 by Roman Kryvolapov
 **/
package com.digitall.eid.data.database.dao

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import androidx.room.Transaction
import com.digitall.eid.data.models.database.PermissionsEntity
import kotlinx.coroutines.flow.Flow

@Dao
interface PermissionsDao {

    @Query("SELECT * FROM permissions")
    fun subscribeToPermissions(): Flow<List<PermissionsEntity>>

    @Transaction
    fun replacePermissions(list: List<PermissionsEntity>) {
        deletePermissions()
        savePermissions(list)
    }

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    fun savePermissions(list: List<PermissionsEntity>)

    @Query("DELETE FROM permissions")
    fun deletePermissions()
}