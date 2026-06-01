import {Pencil, Trash2} from "lucide-react";
import type {CustomerDto} from "../../types/customer.ts";

interface CustomerTableRowProps {
  customer: CustomerDto;
  isSelected: boolean;
  onSelect: (id: number) => void;
  onEdit: (id: number) => void;
  onDelete: (id: number) => void;
}

export function CustomerTableRow({customer, isSelected, onSelect, onEdit, onDelete}: CustomerTableRowProps) {
  return (
    <tr
      onClick={() => onSelect(customer.id)}
      className={`cursor-pointer transition-colors ${
        isSelected ? "bg-amber-50/50 font-medium" : "hover:bg-gray-50/50"
      }`}
    >
      <td className="p-3 text-center font-mono text-gray-400">#{customer.id}</td>

      <td className="p-3 text-gray-900 font-medium">{customer.name}</td>

      <td className="p-3 text-gray-500">{customer.email}</td>

      <td className="p-3 font-mono text-amber-700 font-bold">{customer.bonusPoints} pts</td>

      <td className="p-3 text-center" onClick={(e) => e.stopPropagation()}>
        <div className="flex items-center justify-center gap-2">
          <button
            onClick={() => onEdit(customer.id)}
            className="p-1 text-gray-400 hover:text-amber-600 hover:bg-amber-50 rounded transition-colors"
            title="Edit Customer"
          >
            <Pencil className="w-4 h-4"/>
          </button>

          <button
            onClick={() => onDelete(customer.id)}
            className="p-1 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
            title="Delete Customer"
          >
            <Trash2 className="w-4 h-4"/>
          </button>
        </div>
      </td>
    </tr>
  );
}