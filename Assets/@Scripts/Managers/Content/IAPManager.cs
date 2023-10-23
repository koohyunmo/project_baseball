using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager :  IStoreListener
{
    [Header("Product ID")]
    public readonly string productId_1_id = "1_dollar_star";
    public readonly string productId_2_id = "2_dollar_star";
    public readonly string productId_3_id = "3_dollar_star";

    [Header("Cache")]
    private IStoreController storeController; //���� ������ �����ϴ� �Լ� ������
    private IExtensionProvider storeExtensionProvider; //���� �÷����� ���� Ȯ�� ó�� ������

    public void Init()
    {
        Debug.Log("IAP INIT");
        InitUnityIAP(); //Start ������ �ʱ�ȭ �ʼ�
    }

    /* Unity IAP�� �ʱ�ȭ�ϴ� �Լ� */
    private void InitUnityIAP()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        /* ���� �÷��� ��ǰ�� �߰� */
        builder.AddProduct(productId_1_id, ProductType.Consumable, new IDs() { { productId_1_id, GooglePlay.Name } });
        builder.AddProduct(productId_2_id, ProductType.Consumable, new IDs() { { productId_2_id, GooglePlay.Name } });
        builder.AddProduct(productId_3_id, ProductType.Consumable, new IDs() { { productId_3_id, GooglePlay.Name } });

        UnityPurchasing.Initialize(this, builder);
    }

    /* �����ϴ� �Լ� */
    public void Purchase(string productId)
    {
        Product product = storeController.products.WithID(productId); //��ǰ ����

        if (product != null && product.availableToPurchase) //��ǰ�� �����ϸ鼭 ���� �����ϸ�
        {
            storeController.InitiatePurchase(product); //���Ű� �����ϸ� ����
        }
        else //��ǰ�� �������� �ʰų� ���� �Ұ����ϸ�
        {
            Debug.Log("��ǰ�� ���ų� ���� ���Ű� �Ұ����մϴ�");
        }
    }

    #region Interface
    /* �ʱ�ȭ ���� �� ����Ǵ� �Լ� */
    public void OnInitialized(IStoreController controller, IExtensionProvider extension)
    {
        Debug.Log("�ʱ�ȭ�� �����߽��ϴ�");

        storeController = controller;
        storeExtensionProvider = extension;
    }

    /* �ʱ�ȭ ���� �� ����Ǵ� �Լ� */
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("�ʱ�ȭ�� �����߽��ϴ�");
    }

    /* ���ſ� �������� �� ����Ǵ� �Լ� */
    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.Log("���ſ� �����߽��ϴ�");
    }

    /* ���Ÿ� ó���ϴ� �Լ� */
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {

        if (args.purchasedProduct.definition.id == productId_1_id)
        {
            /* test_id ���� ó�� */
            Managers.Game.GetStar(2000);
        }
        else if (args.purchasedProduct.definition.id == productId_2_id)
        {
            Debug.Log("4500");
            Managers.Game.GetStar(4000);
        }
        else if(args.purchasedProduct.definition.id ==  productId_3_id)
        {
            Debug.Log("7000");
            Managers.Game.GetStar(7000);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}