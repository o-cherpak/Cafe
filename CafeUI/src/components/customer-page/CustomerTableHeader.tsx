export function CustomerTableHeader() {
  return (
    <thead className="bg-gray-50 border-b border-gray-200 text-xs font-bold text-gray-400 uppercase tracking-wider">
    <tr>
      <th className="p-3 w-16 text-center">ID</th>
      <th className="p-3">Customer Name</th>
      <th className="p-3">Email Address</th>
      <th className="p-3">Bonus Balance</th>
      <th className="p-3 w-24 text-center">Actions</th>
    </tr>
    </thead>
  );
}