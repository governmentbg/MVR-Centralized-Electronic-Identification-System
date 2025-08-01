//
//  Parameters+Extension.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 16.10.23.
//

import Foundation
import Alamofire


extension Parameters {
    mutating func append(_ dictionary: Parameters?) {
        guard let dict = dictionary else { return }
        
        for (key, value) in dict {
            let _ = autoreleasepool(invoking: {
                if !(value is NSNull) {
                    self[key] = value
                }
            })
        }
    }
}
