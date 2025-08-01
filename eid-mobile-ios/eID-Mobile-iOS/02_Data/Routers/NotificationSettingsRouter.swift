//
//  NotificationSettingsRouter.swift
//  eID-Mobile-iOS
//
//  Created by Sunay Gyultekin on 11.10.23.
//

import Foundation
import Alamofire


enum NotificationSettingsRouter: URLRequestBuilder {
    // MARK: - Endpoints
    case getNotificationChannels(input: GetNotificationChannelsRequest)
    case getSelectedNotificationChannels(input: GetNotificationChannelsRequest)
    case setSelectedNotificationChannels(input: SetSelectedNotificationChannelsRequest)
    case getNotificationTypes(input: GetNotificationTypesRequest)
    case getDeactivatedNotificationTypes(input: GetNotificationTypesRequest)
    case setDeactivatedNotificationTypes(input: SetDeactivatedNotificationTypesRequest)
    
    // MARK: - BaseURL
    var mainURL: URL  {
        return URL(string: AppConfiguration.get(keychainKey: ServiceDomain.PG.baseUrl) as! String)!
    }
    
    // MARK: - Path
    internal var path: String {
        switch self {
        case .getNotificationChannels:
            return "pan/api/v1/NotificationChannels"
        case .getSelectedNotificationChannels:
            return "pan/api/v1/NotificationChannels/selected"
        case .setSelectedNotificationChannels:
            return "pan/api/v1/NotificationChannels/selection"
        case .getNotificationTypes:
            return "pan/api/v1/Notifications"
        case .getDeactivatedNotificationTypes:
            return "pan/api/v1/Notifications/deactivated"
        case .setDeactivatedNotificationTypes:
            return "pan/api/v1/Notifications/deactivate"
        }
    }
    
    // MARK: - Method
    internal var method: HTTPMethod {
        switch self {
        case .getNotificationChannels,
                .getSelectedNotificationChannels,
                .getNotificationTypes,
                .getDeactivatedNotificationTypes:
            return .get
        case .setSelectedNotificationChannels,
                .setDeactivatedNotificationTypes:
            return .post
            
        }
    }
    
    // MARK: - Encoding
    var encoding: ParameterEncoding {
        switch method {
        case .get:
            return URLEncoding.queryString
        case .post:
            return KeylessParameterEncoding.default
        default:
            return JSONEncoding.default
        }
    }
    
    // MARK: - Parameters
    internal var parameters: Parameters? {
        var params = defaultParams
        switch self {
        case .getNotificationChannels(let input):
            params.append(input.toDict())
        case .getSelectedNotificationChannels(let input):
            params.append(input.toDict())
        case .setSelectedNotificationChannels(let input):
            params.append(input.toDict())
        case .getNotificationTypes(let input):
            params.append(input.toDict())
        case .getDeactivatedNotificationTypes(let input):
            params.append(input.toDict())
        case .setDeactivatedNotificationTypes(let input):
            params.append(input.toDict())
        }
        return params
    }
    
    // MARK: - Headers
    var headers: HTTPHeaders?  {
        var headers: HTTPHeaders = []
        headers.add(HTTPHeader.contentType("application/json"))
        headers.add(HTTPHeader(name: "Cookie", value: "KEYCLOAK_LOCALE=bg"))
        if let bearerToken = StorageManager.keychain.getFor(key: .authToken) {
            headers.add(HTTPHeader.authorization(bearerToken: bearerToken))
        }
        return headers
    }
}
