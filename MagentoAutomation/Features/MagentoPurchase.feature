Feature: Magento Purchase
  As a registered user
  I want to purchase products from Magento store
  So that I can verify the order submission process

Scenario: Purchase multiple products and verify order
  Given I am logged in as a registered user
  When I add 2 jackets and 1 pants to the cart
  And I proceed to checkout
  Then I verify the order summary
  When I enter a valid delivery address
  And I select a delivery method
  And I place the order
  Then I verify the order in My Orders