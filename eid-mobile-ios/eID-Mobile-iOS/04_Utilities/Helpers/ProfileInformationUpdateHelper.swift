//
//  ProfileInformationUpdateHelper.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 2.04.25.
//


enum ProfileInformationUpdateHelper {
    static func change(citizenEID: CitizenEID?,
                       onSuccess: @escaping () -> (),
                       onFailure: @escaping (Error) -> ()) {
        let request = ChangeCitizenInformationRequest(
            firstName: citizenEID?.firstName ?? "",
            secondName: citizenEID?.secondName ?? "",
            lastName: citizenEID?.lastName ?? "",
            firstNameLatin: citizenEID?.firstNameLatin ?? "",
            secondNameLatin: citizenEID?.secondNameLatin ?? "",
            lastNameLatin: citizenEID?.lastNameLatin ?? "",
            phoneNumber: citizenEID?.phoneNumber ?? "",
            is2FaEnabled: citizenEID?.is2FaEnabled ?? false
        )
        CitizenRouter.changeInformation(input: request)
            .send(ServerStatusResponse.self) { response in
                switch response {
                case .success(let response):
                    guard let _ = response else { return }
                    onSuccess()
                    
                case .failure(let error):
                    onFailure(error)
                }
            }
    }
}
