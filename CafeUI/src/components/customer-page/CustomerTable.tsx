import type {CustomerDto} from "../../types/customer.ts";
import {CustomerTableHeader} from "./CustomerTableHeader";
import {CustomerTableRow} from "./CustomerTableRow";

interface CustomerTableProps {
  customers: CustomerDto[];
  selectedId: number | null;
  onSelect: (id: number) => void;
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
}

export function CustomerTable({customers, selectedId, onSelect, onEdit, onDelete}: CustomerTableProps) {
  return (
    <div className="border border-gray-200 rounded-xl overflow-hidden bg-white shadow-sm">
      <table className="w-full text-left border-collapse">
        <CustomerTableHeader/>

        <tbody className="divide-y divide-gray-100 text-xs">
        {customers.map((customer) => (
          <CustomerTableRow
            key={customer.id}
            customer={customer}
            isSelected={selectedId === customer.id}
            onSelect={onSelect}
            onEdit={onEdit}
            onDelete={onDelete}
          />
        ))}
        </tbody>
      </table>
    </div>
  );
}
