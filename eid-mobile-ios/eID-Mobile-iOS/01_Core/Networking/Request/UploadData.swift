//
//  UploadData.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 3.10.23.
//

import Foundation


struct UploadData {
    // MARK: - Properties
    var tag: Int = 0
    var data: Data
    var fileName: String
    var mimeType: String
    var name: String
    var sizeInBytes: Int {
        return data.count
    }
    
    // MARK: - Init
    init(data: Data, fileName: String) {
        self.data = data
        self.fileName = fileName
        mimeType = "image/jpeg"
        name = "content"
    }
}
