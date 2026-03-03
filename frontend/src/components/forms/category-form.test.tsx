import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { CategoryForm } from "@/components/forms/category-form";

describe("CategoryForm", () => {
  it("shows description validation message when missing", async () => {
    const user = userEvent.setup();
    render(<CategoryForm onSubmit={vi.fn()} />);

    const [nameInput] = screen.getAllByRole("textbox");

    await user.type(nameInput, "Eletrônicos");

    expect(screen.getByRole("button", { name: "Adicionar" })).toBeDisabled();
  });

  it("submits when form is valid", async () => {
    const user = userEvent.setup();
    const onSubmit = vi.fn();
    render(<CategoryForm onSubmit={onSubmit} />);

    const [nameInput, descriptionInput] = screen.getAllByRole("textbox");

    await user.type(nameInput, "Eletrônicos");
    await user.type(descriptionInput, "Categoria para produtos eletrônicos");

    const submitButton = screen.getByRole("button", { name: "Adicionar" });
    expect(submitButton).toBeEnabled();

    await user.click(submitButton);

    expect(onSubmit).toHaveBeenCalledTimes(1);
    expect(onSubmit.mock.calls[0][0]).toEqual({
      name: "Eletrônicos",
      description: "Categoria para produtos eletrônicos",
    });
  });
});
