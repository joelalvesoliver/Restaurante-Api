public UsuarioController() { }


[Httppost]
[producesResponseType(StatusCodes.Status201Created)]
[producesResponseType(StatusCodes.Status400BadRequest)]
[producesResponseType(StatusCodes.Status403Forbidden)]

public IActionResult CadastrarUsuario([FromBody] FuncionarioDTO funcionarioDTO)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    // Lógica para cadastrar o usuário
    // ...

    return CreatedAtAction(nameof(CadastrarUsuario), new { id = /* id do usuário criado */ }, /* objeto do usuário criado */);
}

