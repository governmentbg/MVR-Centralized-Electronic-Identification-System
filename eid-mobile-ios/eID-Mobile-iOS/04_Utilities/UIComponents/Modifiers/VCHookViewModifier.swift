//
//  VCHookViewModifier.swift
//  eID-Mobile-iOS
//
//  Created by Teodora Georgieva on 12.07.24.
//

import SwiftUI


class VCHookViewController: UIViewController {
    var onViewWillAppear: ((UIViewController) -> Void)?
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        onViewWillAppear?(self)
    }
}

struct VCHookView: UIViewControllerRepresentable {
    typealias UIViewControllerType = VCHookViewController
    let onViewWillAppear: ((UIViewController) -> Void)
    func makeUIViewController(context: Context) -> VCHookViewController {
        let vc = VCHookViewController()
        vc.onViewWillAppear = onViewWillAppear
        return vc
    }
    func updateUIViewController(_ uiViewController: VCHookViewController, context: Context) {
    }
}

struct VCHookViewModifier: ViewModifier {
    let onViewWillAppear: ((UIViewController) -> Void)
    func body(content: Content) -> some View {
        content
            .background {
                VCHookView(onViewWillAppear: onViewWillAppear)
            }
    }
}

extension View {
    func onViewWillAppear(perform onViewWillAppear: @escaping ((UIViewController) -> Void)) -> some View {
        modifier(VCHookViewModifier(onViewWillAppear: onViewWillAppear))
    }
}
