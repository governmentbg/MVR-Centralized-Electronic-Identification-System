//
//  VerifyLoginRequestHelper.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.04.25.
//


enum VerifyLoginRequestHelper {
    // MARK: - Static Methods
    static func verifyLogin(onSuccess: @escaping () -> (), onFailure: @escaping (any Error) -> ()) {
        guard let instanceId = UserManager.getMobileAppInstanceId(),
              let firebaseId = UserManager.getFirebaseId()
        else { return }
        let request = VerifyLoginRequest(mobileApplicationInstanceId: instanceId,
                                         firebaseId: firebaseId)
        AuthRouter.verifyLogin(input: request).send(VerifyLoginResponse.self) { response in
            switch response {
            case .success(let response):
                onSuccess()
#if DEBUG
                print("Verify login: \(response?.message ?? "")")
#endif
            case .failure(let error):
#if DEBUG
                print("Verify login: Error - \(error.localizedDescription)")
#endif
                onFailure(error)
            }
        }
    }
}
