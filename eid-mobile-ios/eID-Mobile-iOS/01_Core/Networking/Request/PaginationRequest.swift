//
//  PaginationRequest.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation


/** Protocol with default parameters required for requests with pagination */
protocol PaginationRequest where Self: Codable {
    // MARK: - Properties
    var pageSize: Int { get }
    var pageIndex: Int { get }
}


protocol PaginationMPOZEIRequest where Self: Codable {
    // MARK: - Properties
    var size: Int { get }
    var page: Int { get }
}
