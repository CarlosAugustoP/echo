// SPDX-License-Identifier: MIT
pragma solidity ^0.8.18;

/**
 * @title EchoProjectEscrow
 * @dev Contrato de custódia para doações de projetos sociais.
 */
contract EchoProjectEscrow {
    address public platformAdmin; // Endereço da sua API (EchoProject)
    uint256 public totalDonated;
    
    // Eventos para transparência (Essenciais para o TCC)
    event DonationReceived(address indexed donor, uint256 amount);
    event FundsReleased(address indexed supplier, uint256 amount);

    // Modifier para garantir que apenas sua API chame certas funções
    modifier onlyPlatform() {
        require(msg.sender == platformAdmin, "Acesso negado: Apenas a plataforma Echo pode executar esta acao.");
        _;
    }

    /**
     * @dev O construtor define quem é o administrador (a conta da sua API)
     */
    constructor(address _platformAdmin) {
        platformAdmin = _platformAdmin;
    }

    /**
     * @dev Função para receber doações. 
     * O dinheiro entra no "limbo" (saldo do contrato) ao ser enviado para cá.
     */
    receive() external payable {
        require(msg.value > 0, "A doacao deve ser maior que zero.");
        totalDonated += msg.value;
        emit DonationReceived(msg.sender, msg.value);
    }

    /**
     * @dev Libera fundos para um fornecedor específico.
     * @param _supplier Carteira do fornecedor que receberá o dinheiro.
     * @param _amount Valor em Wei a ser transferido.
     */
    function releaseFunds(address payable _supplier, uint256 _amount) external onlyPlatform {
        require(address(this).balance >= _amount, "Saldo insuficiente no contrato.");
        require(_supplier != address(0), "co de fornecedor invalido.");

        // Realiza a transferência
        (bool success, ) = _supplier.call{value: _amount}("");
        require(success, "Falha ao transferir fundos para o fornecedor.");

        emit FundsReleased(_supplier, _amount);
    }

    /**
     * @dev Retorna o saldo atual "preso" no contrato.
     */
    function getBalance() public view returns (uint256) {
        return address(this).balance;
    }
}