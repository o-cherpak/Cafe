const link_style =
  "px-3 py-2 text-sm font-medium rounded-lg " +
  "text-gray-700 hover:bg-amber-50 hover:text-amber-600 transition-colors block";

export function Sidebar() {
  return (
    <aside className="w-58 flex-shrink-0">
      <div className="bg-white rounded-xl border border-gray-200 p-4 shadow-sm sticky top-20">
        <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider mb-4 px-2">
          Navigation
        </h3>

        <div className="flex flex-col">
          <a href="#auth" className={link_style}>
            Authentication
          </a>

          <a href="#customers" className={link_style}>
            Customers
          </a>

          <a href="#menu" className={link_style}>
            Menu Items
          </a>

          <a href="#orders" className={link_style}>
            Orders
          </a>

          <a href="#promotions" className={link_style}>
            Promotions
          </a>

          <a href="#customer-promotions" className={link_style}>
            Customer Promo
          </a>
        </div>
      </div>
    </aside>
  );
}