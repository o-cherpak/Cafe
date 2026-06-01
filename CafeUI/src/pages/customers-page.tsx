import {type FormEvent, useState} from "react";
import type {CustomerDto} from "../types/customer.ts";
import {CustomerTable} from "../components/customer-page/CustomerTable.tsx";
import {CustomerHeader} from "../components/customer-page/CustomerHeader.tsx";

export default function CustomerPage() {
  // @ts-ignore
  const [customers, setCustomers] = useState<CustomerDto[]>([
    {id: 1, name: "Oleksandr Cherpak", email: "cherpak.oleksandr@gmail.com", bonusPoints: 150},
    {id: 2, name: "John Doe", email: "john.doe@cafe.com", bonusPoints: 20},
    {id: 3, name: "Alice Smith", email: "alice.s@gmail.com", bonusPoints: 0},
  ]);

  const [selectedId, setSelectedId] = useState<number | null>(customers[0]?.id || null);
  const [searchEmail, setSearchEmail] = useState("");

  const handleAddNew = () => {
    alert("");
  };

  const handleEdit = (id: number) => {
    alert(`${id}] `);
  };

  const handleDelete = (id: number) => {
    alert(`${id}] `);
  };

  const handleSearchSubmit = (e: FormEvent) => {
    e.preventDefault();
    alert(`${searchEmail}`);
  };

  return (
    <div className="space-y-6">
      <CustomerHeader
        searchEmail={searchEmail}
        onSearchChange={setSearchEmail}
        onSearchSubmit={handleSearchSubmit}
        onAddNew={handleAddNew}
      />

      <CustomerTable
        customers={customers}
        selectedId={selectedId}
        onSelect={setSelectedId}
        onEdit={handleEdit}
        onDelete={handleDelete}
      />
    </div>
  );
}