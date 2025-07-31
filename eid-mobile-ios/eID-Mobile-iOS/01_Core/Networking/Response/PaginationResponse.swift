//
//  PaginationResponse.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation


/** Protocol with default parameters required for requests with pagination */
protocol PaginationResponse where Self: Codable {
    // MARK: - Properties
    var pageIndex: Int { get }
    var totalItems: Int { get }
}

protocol PaginationMPOZEIResponse where Self: Codable {
    // MARK: - Properties
    var number: Int { get }
    var totalPages: Int { get }
    var totalElements: Int { get }
}
